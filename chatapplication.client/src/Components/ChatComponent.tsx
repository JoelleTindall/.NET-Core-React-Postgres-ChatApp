// src/components/ChatComponent.tsx
import React, { useEffect, useState } from 'react';
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

    useEffect(() => {
        const fetchChats = async () => {
            try {
                const response = await fetch('/chat'); // Adjust if your API base URL is different
                if (!response.ok) throw new Error('Failed to fetch chats');

                const data: Chat[] = await response.json();
                setChats(data);
            } catch (error) {
                console.error('Error fetching chat messages:', error);
            }
        };

        fetchChats();
    }, []);

                //    <div class="username">@item.UserName</div>
                //<div class="date">@item.CreatedAt.ToString("MM/dd/yyyy HH:mm")</div>
                //<div class="message">
                //    <p class="messagetext">@item.Message</p>
    //</div>
    //<strong>{chat.user?.userName}</strong>: { chat.message }



    //<table key={chat.id}>

    //    <tr>
    //        <td>{chat.user.avatar?.filePath}</td>
    //        <td>{chat.user?.userName}</td>
    //        <td>{chat.createdAt}</td>
    //    </tr>
    //    <tr>
    //        <td>{chat.message}</td>
    //    </tr>

    //</table>
    return (
        <div className="wrapper">
            <h2>Chat Messages</h2>
            <div className="chatview" id="ChatView">

            
                {chats.map(chat => (


                    <div key={chat.id}>

                        
                            <div className="avatar">{chat.user.avatar?.filePath}</div>
                            <div className="username">{chat.user?.userName}</div>
                            <div className="date">{chat.createdAt}</div>
                        <div className="message"><p className="messagetext">{chat.message}</p></div>
                        
                       
                    </div>
                ))}
                
            </div>
        </div>
    );
};

export default ChatComponent;
