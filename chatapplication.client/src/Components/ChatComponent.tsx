// src/components/ChatComponent.tsx
import React, { useEffect, useState, useRef } from 'react';
import { format } from 'date-fns';
import { useClickAway } from 'react-use';
import { AnimatePresence, motion } from 'framer-motion';
import { Rotate as Hamburger } from 'hamburger-react';
import LogoutLink from './LogoutLink';
import { AuthorizedUser } from './AuthorizeView';
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
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    //const [avatars, setAvatars] = useState<Avatar[]>([]);
    const [isOpen, setOpen] = useState(false);
    const [isEditOpen, setEditOpen] = useState(false);
    const ref = useRef(null);

    useClickAway(ref, () => setOpen(false));

    //const [isOpen, setOpen] = useState(false)

    useEffect(() => {
        const fetchChats = async () => {
            try {
                const response = await fetch('/chat');
                if (!response.ok) throw new Error('Failed to fetch chats');

                const data: Chat[] = await response.json();
                setChats(data);
            } catch (error) {
                console.error('Error fetching chat messages:', error);
            }
        };

        fetchChats();

        const fetchCurrentUser = async () => {
            try {
                const res = await fetch('/pingauth', { credentials: 'include' });
                if (res.ok) {
                    const user = await res.json();
                    setCurrentUserId(user.email); // id is returned from updated pingauth endpoint
                }
            } catch (err) {
                console.error('Failed to get current user:', err);
            }
        };

        fetchCurrentUser();

            
    }, []);

    const handleSendMessage = async () => {
        if (!newMessage.trim()) return;

        try {
            const response = await fetch('/chat', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({ message: newMessage })
            });

            if (!response.ok) throw new Error('Failed to send message');

            const newChat: Chat = await response.json();

            setChats((prevChats) => [...prevChats, newChat]);
            setNewMessage('');
        } catch (error) {
            console.error('Error sending message:', error);
        }
    };

    


    return (
        
        <div className="wrapper third-color">



            <ul className="chat-header">

                <li className="a"><h2>Chat It Up!</h2></li>

                <li></li>
                <li className="b">
                    <div className="hamburger-btn" ><Hamburger toggled={isOpen} toggle={setOpen}/></div> 

                </li>

            </ul> 
         
            {isOpen && (<div className="menu-wrapper" >

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
                                <li><p>Other Option</p></li>
                                <li>
                                    <LogoutLink>
                                        Logout (<AuthorizedUser value="email" />)
                                    </LogoutLink>
                                </li>
                            </ul>
                        </motion.div>
                   
                </AnimatePresence>
            </div>)}
     

            
            <div className="chatview second-color" id="ChatView">

            
                {chats.map(chat => {
                    const isCurrentUser = chat.user.userName === currentUserId;
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
