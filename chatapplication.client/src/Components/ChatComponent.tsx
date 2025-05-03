import React, { useEffect, useState, useRef } from 'react';
import { HubConnectionBuilder, HubConnection, HttpTransportType } from '@microsoft/signalr';
import { format } from 'date-fns';
import { useClickAway } from 'react-use';
import { AnimatePresence, motion } from 'framer-motion';
import { Rotate as Hamburger } from 'hamburger-react';
import LogoutLink from './LogoutLink';
import { jwtDecode } from 'jwt-decode';
import AvatarPicker from './AvatarPicker';
import '../assets/styles/ChatStyle.css';
import '../assets/styles/MenuStyle.css';
import AdminComponent from './AdminComponent';
import AdminView from './AdminView';
import { useIdleTimer } from 'react-idle-timer'; 
import { useAuth } from "../Context/AuthContext";
interface Avatar {
    id: string;
    filePath: string;
}

interface User {
    id: string;
    userName: string;
    avatar?: Avatar;
}

interface Chat {
    id: string;
    message: string;
    createdAt: string;
    user: User;
    isDeleted?: boolean;
}

const ChatComponent: React.FC = () => {
    const [chats, setChats] = useState<Chat[]>([]);
    const [newMessage, setNewMessage] = useState<string>('');
    const [currentUser, setUsername] = useState<string | null>(null);
    const [currentUserId, setUserId] = useState<string | null>(null);
    const [currentUserAdmin, setCurrentUserAdmin] = useState<boolean | null>(false);
    const [isOpen, setOpen] = useState(false);
    const [hasMore, setHasMore] = useState(true);
    const [loading, setLoading] = useState(true);
    const isFirstLoad = useRef(true);
    const isConnected = useRef(false);
    const connectionRef = useRef<HubConnection | null>(null);
    const menuRef = useRef(null);
    const scrollContainerRef = useRef<HTMLDivElement>(null);
    const scrollAnchorRef = useRef<HTMLDivElement>(null);
    const scrolledUp = useRef(true);
    const { logout } = useAuth();

    //this is buggy and needs to be reworked
    useClickAway(menuRef, () => setOpen(false));

    //auto logs out user after 5 minutes of inactivity
    useIdleTimer({
        timeout: 5 * 60 * 1000, // 5 mins
        onIdle: logout,
        debounce: 500, // .5 secs
    });

    // Get current user info
    useEffect(() => {
        console.log('Getting current user info...');
        const token = localStorage.getItem('token');

        if (token) {
            const decoded = jwtDecode<{ sub: string; userId: string; isAdmin: string }>(token);
            setUsername(decoded.sub);
            setUserId(decoded.userId);
            setCurrentUserAdmin(decoded.isAdmin === 'true');

        }

    }, []);

    // Initial fetch
    const fetchChats = async () => {
        console.log('Fetching chats...');
        try {
            const response = await fetch('/api/chat/');
            if (!response.ok) throw new Error('Failed to fetch chats');
            const data: Chat[] = await response.json();
            setChats(data);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching chat messages:', error);
        }
    };

    // SignalR setup
    const connectSignalR = async () => {
        if (isConnected.current) return;
        console.log('connecting signalr...');
        const connection = new HubConnectionBuilder()
            .withUrl('/chathub', {
                transport: HttpTransportType.WebSockets,
                skipNegotiation: true,
                withCredentials: true,
            })
            .withAutomaticReconnect()
            .build();

        connectionRef.current = connection;

        connection.off('ReceiveMessage');

        connection.on('ReceiveMessage', (userId, userName, message, avatarUrl, createdAt) => {
            const newChat: Chat = {
                id: crypto.randomUUID(),
                message,
                createdAt,
                user: {
                    id: userId,
                    userName,
                    avatar: { id: '', filePath: avatarUrl },
                },
            };
            setChats((prev) => [...prev, newChat]);
        });

        connection.on('ToggleDelete', (chatId, isDeleted) => {
            if (isDeleted) {
                // update chats to exclude deleted chats
                setChats((prevChats) => prevChats.filter((chat) => chat.id !== chatId));
            } else {
                // update chats to include chats that were recently restored
                fetchChats();
            };

        });

        connection.on('UserBanned', (userId, isBanned) => {
            if (isBanned) {
                // update chats to exclude banned user
                setChats((prevChats) => prevChats.filter((chat) => chat.user.id !== userId));
                //logs the banned user out
                if (currentUserId === userId) {
                    logout();
                };
            } else {
                // update chats to include chats from users that were recently unbanned
                fetchChats();
            }
        });

        await connection.start();
        isConnected.current = true;
    };

    useEffect(() => {
        if (!currentUserId || isConnected.current) return;
        console.log('fetch chats, connect signalr...');
        fetchChats();
        connectSignalR();


        return () => {
            if (connectionRef.current) {
                connectionRef.current.stop();
                isConnected.current = false;
            }
        };
    }, [currentUserId]);

    // Scroll to bottom
    const scrollToBottom = () => {
        scrollAnchorRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    // Infinite scroll to top
    const handleScrollToTop = async () => {
        const container = scrollContainerRef.current;
        if (!container || loading || !hasMore) return;

        if (container.scrollTop <= 0) {
            const oldest = chats[0];
            const beforeDate = new Date(oldest.createdAt);
            beforeDate.setMilliseconds(beforeDate.getMilliseconds() - 1);

            setLoading(true);

            try {
                const res = await fetch(
                    `/api/chat?before=${encodeURIComponent(beforeDate.toISOString())}&pageSize=10`
                );
                const olderChats: Chat[] = await res.json();

                if (olderChats.length === 0) {
                    setHasMore(false);
                } else {
                    setChats((prev) => [...olderChats, ...prev]);

                }
            } catch (err) {
                console.error('Error loading older chats:', err);
            }

            setLoading(false);
        }
    };

    // Auto-scroll on first load
    useEffect(() => {
        console.log('auto scroll first load...');
        if (chats.length > 0 && isFirstLoad.current) {
            scrollToBottom();
            isFirstLoad.current = false;
        }
    }, [chats]);


    // determine if user is scrolled halfway up

    useEffect(() => {
        const container = scrollContainerRef.current;

        const handleScroll = () => {
            if (container) {
                const { scrollTop, scrollHeight, } = container;
                const halfwayPoint = scrollHeight / 2;

                scrolledUp.current = scrollTop > halfwayPoint;

            }
        };

        container?.addEventListener('scroll', handleScroll);

        return () => {
            container?.removeEventListener('scroll', handleScroll);
        };
    }, []);

    // Scroll to bottom when new chat added if menu closed and user is not scrolled more than halfway
    useEffect(() => {
        if (!isFirstLoad.current && !isOpen && scrolledUp.current) {
            scrollToBottom();
        }
    }, [chats.length, isOpen]);


    // Send message through SignalR
    const handleSendMessage = async () => {
        if (!newMessage.trim() || !connectionRef.current || !currentUserId) return;
        try {
            await connectionRef.current.invoke('SendMessage', parseInt(currentUserId), newMessage);
            setNewMessage('');
        } catch (err) {
            console.error('Error sending message:', err);
        }
    };

    return (
        <div className="wrapper third-color">
            
                <ul className="chat-header">
                    <li className="a">
                        <h2>{currentUserAdmin ? "Admin Chat" : "Chat It Up!"}</h2>
                    </li>
                    <li></li>
                    <li className="b">
                        <div className="hamburger-btn">
                        <Hamburger toggled={isOpen} toggle={setOpen} disabled={loading } />
                        </div>
                    </li>
                </ul>

            {isOpen && (
                <div className="menu-wrapper" ref={menuRef}>
                    <AnimatePresence>
                        <motion.div
                            className="menu-overlay"
                            initial={{ opacity: 0, y: -10 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: -10 }}
                            transition={{ duration: 0.2 }}
                        >
                            <ul className="menu-list">
                                {currentUserAdmin && <li><AdminView connection={connectionRef.current} currentuser={currentUserId} /></li>}
                                <li><AvatarPicker /></li>
                                <li> <LogoutLink>Logout {currentUser}</LogoutLink></li>
                            </ul>
                        </motion.div>
                    </AnimatePresence>
                </div>
            )}

            <div
                className="chatview second-color"
                id="ChatView"
                ref={scrollContainerRef}
                onScroll={handleScrollToTop}
            >{loading ? <div className="loading">loading</div> : ''}
                {chats.map((chat) => {
                    const isCurrentUser = chat.user.id === currentUserId;
                    return (
                        <div
                            className={`chatmessage ${isCurrentUser ? 'own-message' : ''}`}
                            key={chat.id}
                        >
                            <div className="chat-row">
                                <div className="avatar">
                                    <img src={`/${chat.user.avatar?.filePath}`} alt="avatar" />
                                </div>
                                <div className="chat-bubble">
                                    <div className="username-date">
                                        <div className="username first-color">
                                            <span>{currentUserAdmin ? <AdminComponent selectedChat={{ userid: chat.user.id, username: chat.user.userName, id: chat.id, connection: connectionRef.current, currentuser: currentUserId }} /> : chat.user.userName} </span>
                                        </div>
                                        <div className="date">
                                            <span>
                                                {format(new Date(chat.createdAt), 'M/dd/yy HH:mm')}
                                            </span>
                                        </div>
                                    </div>
                                    <div className="message first-color">
                                        <p className="messagetext">{chat.message}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    );
                })}
                <div ref={scrollAnchorRef}></div>
            </div>

            <div className="chat-input-container">
                <textarea
                    placeholder="Type your message..."
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter') handleSendMessage();
                    }}
                    className="chat-input"
                />
                <button onClick={handleSendMessage} className="send-button fourth-color">
                    Send
                </button>
            </div>

        </div>
    );

};

export default ChatComponent;
