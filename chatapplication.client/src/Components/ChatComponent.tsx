import React, { useEffect, useState, useRef } from 'react';
import { HubConnectionBuilder, HubConnection, HttpTransportType } from '@microsoft/signalr';
import { format } from 'date-fns';
import { useClickAway } from 'react-use';
import { AnimatePresence, motion } from 'framer-motion';
import { Rotate as Hamburger } from 'hamburger-react';
import LogoutLink from './LogoutLink';
import { jwtDecode } from "jwt-decode";
import AvatarPicker from './AvatarPicker';
import '../assets/styles/ChatStyle.css';
import '../assets/styles/MenuStyle.css';

// Type definitions
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
}

const ChatComponent: React.FC = () => {
    const [chats, setChats] = useState<Chat[]>([]);
    const [newMessage, setNewMessage] = useState<string>('');
    const [currentUser, setUsername] = useState<string | null>(null);
    const [currentUserId, setUserId] = useState<string | null>(null);
    const [isOpen, setOpen] = useState(false);
    const [connection, setConnection] = useState<HubConnection | null>(null);
    //const [token, setToken] = useState('');
    // const [isEditOpen, setEditOpen] = useState(false);

    const ref = useRef(null);
    const scrollRef = useRef<HTMLDivElement>(null);  // references the bottom of the chat container
    const isFirstLoad = useRef(true);  
    const isConnected = useRef(false);  // track whether SignalR is already connected
    const isInitialized = useRef(false); // To avoid effect running multiple times

    useClickAway(ref, () => setOpen(false));

    useEffect(() => {
        const token = localStorage.getItem("token");
        if (token) {
            const decodedToken = jwtDecode<{ sub: string, userId: string }>(token);
            setUsername(decodedToken.sub);
            setUserId(decodedToken.userId);
        }
    }, []);

    // Fetch chat messages
    const fetchChats = async () => {
        try {
            const response = await fetch('/api/chat');
            if (!response.ok) throw new Error('Failed to fetch chats');
            const data: Chat[] = await response.json();
            setChats(data);
        } catch (error) {
            console.error('Error fetching chat messages:', error);
        }
    };



    // set up signalr connection
    const connectSignalR = async () => {
        // Prevent connection if already connected
        if (isConnected.current) return;

        const hubConnection = new HubConnectionBuilder()
            .withUrl('/chathub', {
                transport: HttpTransportType.WebSockets,
                skipNegotiation: true
            })
            .withAutomaticReconnect()
            .build();

        // Remove any existing listeners before adding new ones
        hubConnection.off("ReceiveMessage");

        // Add handler for receiving messages
        hubConnection.on("ReceiveMessage", (userId, userName, message, avatarUrl, createdAt) => {
            const newChat = {
                id: crypto.randomUUID(),
                message,
                createdAt,
                user: {
                    id: userId,
                    userName,
                    avatar: { id: '', filePath: avatarUrl }
                }
            };

            setChats(prevChats => [...prevChats, newChat]);
        });

        // Start the connection
        await hubConnection.start();
        setConnection(hubConnection);
        isConnected.current = true;  // Mark as connected
        console.log("Connected to SignalR hub!");
    };

    // Effect to fetch chats and user on mount
    useEffect(() => {
        if (isInitialized.current) return;
        isInitialized.current = true;

        fetchChats();
        // fetchCurrentUser();
        connectSignalR();

        return () => {
            // Cleanup signalr connection on unmount
            if (connection) {
                connection.stop();
                isConnected.current = false;  // Reset connection flag
                console.log("Disconnected from SignalR hub.");
            }
        };
    }, []); // Empty dependency array ensures effect runs only once

    // Scroll to bottom function
    const scrollToBottom = () => {
        if (scrollRef.current) {
            scrollRef.current.scrollIntoView({ behavior: 'smooth' });
        }
    };

    // Auto scroll when first loaded and when new message arrives
    useEffect(() => {
        if (isFirstLoad.current) {
            // On load, scroll to bottom
            scrollToBottom();
            isFirstLoad.current = false; // After load, stop auto-scrolling
        }
    }, [chats]); //  run when chats changes

    useEffect(() => {
        // scroll to bottom when a new message is added
        if (!isFirstLoad.current && !isOpen) {
            scrollToBottom();
        }
    }, [chats,isOpen]);  // trigger scroll on every chat update

    const handleSendMessage = async () => {
        if (!newMessage.trim() || !connection || !currentUserId) return;
        try {
            await connection.invoke("SendMessage", currentUserId, newMessage);
            setNewMessage('');
        } catch (err) {
            console.error("Error sending message through SignalR:", err);
        }
    };

    return (
        <div className="wrapper third-color">
            <ul className="chat-header">
                <li className="a"><h2>Chat It Up!</h2></li>
                <li></li>
                <li className="b">
                    <div className="hamburger-btn" ><Hamburger toggled={isOpen} toggle={setOpen} /></div>
                </li>
            </ul>

            {isOpen && (
                <div className="menu-wrapper">
                    <AnimatePresence>
                        <motion.div
                            className="menu-overlay"
                            initial={{ opacity: 0, y: -10 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: -10 }}
                            transition={{ duration: 0.2 }}
                        >
                            <ul className="menu-list">
                                <li><p>Edit User</p></li>
                                <li><AvatarPicker /></li>
                                <li>
                                    <LogoutLink>
                                        Logout {currentUser }
                                    </LogoutLink>
                                </li>
                            </ul>
                        </motion.div>
                    </AnimatePresence>
                </div>
            )}

            <div className="chatview second-color" id="ChatView">
                {chats.map(chat => {
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
                                            <span className="username first-color">{chat.user?.userName}</span>
                                        </div>
                                        <div className="date">
                                            <span className="date">
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
                <div ref={scrollRef}></div> {/* This is the anchor element for scrolling */}
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
