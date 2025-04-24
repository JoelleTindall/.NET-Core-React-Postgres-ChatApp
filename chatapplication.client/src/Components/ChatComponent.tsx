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
    const ref = useRef(null);
    const scrollRef = useRef<HTMLDivElement>(null);  // references the bottom of the chat container for scrollin
    const isFirstLoad = useRef(true);  
    const isConnected = useRef(false);  // track if chathub is connected
    const connectionRef = useRef<HubConnection | null>(null);
 
    // Close menu when clicking outside
    useClickAway(ref, () => setOpen(false));


    // Get current user from local storage
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
           // console.log("Fetched chats:", data);

        } catch (error) {
            console.error('Error fetching chat messages:', error);
        }
    };



    // set up signalr connection
    const connectSignalR = async () => {
        // prevent connection if already connected
        if (isConnected.current) return;

        const hubConnection = new HubConnectionBuilder()
            .withUrl('/chathub', {
                transport: HttpTransportType.WebSockets,
                skipNegotiation: true,
                withCredentials: true
            })
            //.configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            
            .build();
        connectionRef.current = hubConnection;

        // Remove existing listeners before adding new ones
        hubConnection.off("ReceiveMessage");

        // handler for receiving messages
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
        isConnected.current = true;  // Mark as connected
    
    };

  // fetch chats from signalr chathub
    useEffect(() => {
        if (!currentUserId || isConnected.current) return;
        
        fetchChats();
        connectSignalR();

        return () => {
            // Cleanup signalr connection on unmount
            if (connectionRef.current) {
                connectionRef.current.stop();
                isConnected.current = false; 
            }
        };
    }, [currentUserId]); // currentUserId to ensure chats displayed correctly

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
        if (!newMessage.trim() || !connectionRef.current || !currentUserId) return;
        try {
            await connectionRef.current.invoke("SendMessage", parseInt(currentUserId), newMessage);
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
