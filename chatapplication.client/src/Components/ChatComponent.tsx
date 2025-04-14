// src/components/ChatComponent.tsx
import React, { useEffect, useState } from 'react';
import { format } from 'date-fns';
import '../assets/styles/ChatStyle.css';
// Type definitions that match your .NET models
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
        <div className="wrapper">
            <h2>Chat Messages</h2>
            <div className="chatview" id="ChatView">

            
                {chats.map(chat => (


                    <table className="chatmessage" key={chat.id}>
                    <tbody>
                        <tr>
                            <td className="avatar"><img src={`/${chat.user.avatar?.filePath}`} alt="avatar"></img></td>
                            <td>
                                    <table>
                                <tbody>
                                <tr>
                                    <td><div className="username">{chat.user?.userName}</div></td>
                                        <td>
                                            <div className="date">
                                                {format(new Date(chat.createdAt), 'MM/dd/yyyy HH:mm')}
                                            </div>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td colSpan={2}>
                                            <div className="message" ><p className="messagetext">{chat.message}</p></div>
                                        </td>
                                            </tr>
                                        </tbody>
                                </table>
                            </td>
                        </tr>
                        
         
                        </tbody>
                    </table>
                ))}
                
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
                
              
                <button onClick={handleSendMessage} className="send-button">
                    Send
                    </button>
                
            </div>
        </div>
    );
};

export default ChatComponent;
