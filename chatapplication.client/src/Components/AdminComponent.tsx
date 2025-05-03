//import { useAuth } from "../Context/AuthContext";
import { useRef, useState } from 'react';
import { useClickAway } from 'react-use';
import { HubConnection } from '@microsoft/signalr';

interface AdminProps {
    selectedChat: {
        id: string;
        username: string;
        userid: string;
        connection: HubConnection | null;
        currentuser: string | null; 
    };
}

const AdminComponent: React.FC<AdminProps> = ({ selectedChat }) => {
    const [isOpen, setOpen] = useState(false);
    const menuRef = useRef(null);

    useClickAway(menuRef, () => setOpen(false));

    const openOptions = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        setOpen(!isOpen);

    };

    const deleteChat = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        handleDeleteMessage();
    };

    // mark message as deleted through SignalR
    const handleDeleteMessage = async () => {
        if (selectedChat.connection) {
            try {
                await selectedChat.connection.invoke('MarkDeleted', parseInt(selectedChat.id));
                console.log("Deleted chat:", selectedChat.id);
                setOpen(false);
            } catch (err) {
                console.error('Error deleting message:', err);
            }
        }
    };

    const banUser = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        handleBanUser();


    }

    // Send message through SignalR
    const handleBanUser = async () => {
        if (selectedChat.connection) {
            try {
                await selectedChat.connection.invoke('MarkBanned', parseInt(selectedChat.userid));
                console.log("Banned user:", selectedChat.username);
                setOpen(false);
            } catch (err) {
                console.error('Error banning user:', err);
            }
        }
    };




    return (
        <>
            <a href="#" onClick={openOptions}>{selectedChat.username}</a>
            {isOpen && (
                <div ref={menuRef} >
                    <ul style={{ listStyleType: 'none', padding: '0' }}>
                        <li>
                            <a href="#" onClick={deleteChat}>Delete Chat</a>
                        </li>
                        {selectedChat.currentuser != selectedChat.userid && ( 
                        <li>
                            <a href="#" onClick={banUser}>Ban User</a>
                        </li>)}
                    </ul>
                </div>
            )}
        </>
    );
}

export default AdminComponent;


