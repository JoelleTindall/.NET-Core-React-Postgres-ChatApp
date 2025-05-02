import { useEffect, useState } from "react";
import { format } from 'date-fns';
import { HubConnection } from '@microsoft/signalr';
import '../assets/styles/AdminStyle.css';

interface AdminProps {

    connection: HubConnection | null;
    currentuser: string | null; 

}

interface User {
    id: string;
    userName: string;
    isAdmin: boolean;
    isBanned: boolean;
}
interface Chat {
    id: string;
    message: string;
    createdAt: string;
    user: User;
    isDeleted: boolean;
}

const AdminView: React.FC<AdminProps> = ({ connection, currentuser }) => {
    const [isOpen, setOpen] = useState(false);
    const [users, setUsers] = useState<User[]>([]);
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [chats, setChats] = useState<Chat[]>([]);


    const fetchUsers = async () => {
        try {
            const response = await fetch('/api/user', {
                method: 'GET',
            });
            if (!response.ok) throw new Error('Failed to fetch users');
            const data: User[] = await response.json();
            setUsers(data);
        } catch (error) {
            console.error('Failed to get users:', error);
        }

    };
    // Fetch users when component mounts
    useEffect(() => {
        fetchUsers();


       

    }, []);

    const showMenu = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        setOpen(prev => !prev);
    };

    const editUser = (e: React.MouseEvent<HTMLAnchorElement>, user: User) => {
        e.preventDefault();
        setSelectedUser(user);
        getUserChats(user.id);
    };

    const makeAdmin = (e: React.MouseEvent<HTMLButtonElement>, user: User) => {
        e.preventDefault();
        handleMakeAdmin(user.id);
    };

    const handleMakeAdmin = async (userId: string) => {
        try {
            const response = await fetch(`/api/user/${userId}/admin`, {
                method: 'GET',
            });
            if (!response.ok) throw new Error('Failed to make admin');
            const data: User = await response.json();
            setSelectedUser(data);
        } catch (error) {
            console.error('Failed to make admin:', error);
        }

    };

    //get click event
    const banUser = (e: React.MouseEvent<HTMLButtonElement>, user: User) => {
        e.preventDefault();
        handleBanUser(user.id);


    };

    // Send message through SignalR
    const handleBanUser = async (userid: string) => {
        if (connection) {
            try {
                await connection.invoke('MarkBanned', parseInt(userid));
                if (selectedUser) { selectedUser.isBanned = !selectedUser.isBanned }
            } catch (err) {
                console.error('Error banning user:', err);
            }
        }
    };

    const getUserChats = async (userId: string) => {
        try {
            const response = await fetch(`/api/chat/${userId}`);
            if (!response.ok) throw new Error('Failed to fetch chats');
            const data: Chat[] = await response.json();
            setChats(data);
        } catch (error) {
            console.error('Error fetching chat messages:', error);
        }
    };


    const deleteChat = (e: React.MouseEvent<HTMLButtonElement>, chat: Chat) => {
        e.preventDefault();

        handleDeleteMessage(chat.id);
    };

    // mark message as deleted through SignalR
    const handleDeleteMessage = async (chatid: string) => {
        if (connection) {
            try {
                await connection.invoke('MarkDeleted', parseInt(chatid));
                if (selectedUser) { getUserChats(selectedUser.id); };

            } catch (err) {
                console.error('Error deleting message:', err);
            }
        }
    };

    const backButton = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        setSelectedUser(null);
        fetchUsers();
        setChats([]);
    };




    const contents = users === undefined
        ? <p><em>Loading...</em></p>
        :
        <table>
            <tbody key=''>
                <tr>
                    <th>Username</th>
                    <th>Status</th>
                    <th>Role</th>
                </tr>
                {users.map(user => {

                    return (


                        <tr key={user.id}>
                            <td>{user.userName}</td>
                            <td className={` userstatus ${user.isBanned ? 'banned' : ''}`}>{user.isBanned ? "Banned" : "Active"}</td>
                            <td className={` userrole ${user.isAdmin ? 'admin' : ''}`}>{user.isAdmin ? "Admin" : "User"}</td>
                            <td><a className="edituser" onClick={(e) => editUser(e, user)}>Edit</a></td>
                        </tr>

                    );
                })}
            </tbody>
        </table>;


    const chatContents = chats === undefined
        ? <p><em>Loading...</em></p>
        :
        <table>
            <tbody>
                <tr className="adminth">
                    <th>Message</th>
                    <th>Date</th>
                    <th>Remove</th>
                </tr>
                {chats.map((chat) => {
                    return (
                        <tr key={chat.id} className={chat.isDeleted ? 'removed' : 'active'}>
                            <td className="usermessage">{chat.message}</td>
                            <td>{format(new Date(chat.createdAt), 'M/dd/yy HH:mm')}</td>

                            <td><button onClick={(e) => deleteChat(e, chat)}>{chat.isDeleted ? "Restore" : "Remove"}</button></td>
                        </tr>
                    );
                })
                }

            </tbody>
        </table>;



    return (

        <div className={`adminwrapper ${isOpen ? 'adminopened' : ''}`}>
            <a className="menuOption" href="#" onClick={showMenu}>
                Admin Tools
            </a>

            {isOpen && (
                <div className="toolswrapper" >
                    {!selectedUser ? (
                        <div className="usernames">

                            {contents}
                        </div>
                    ) : (<>
                           
                        <div className="userdetails">
                            <table>
                                <tbody>
                                    <tr>
                                        <th>Username</th>
                                        <th>Banned</th>
                                        <th>Admin</th>
                                    </tr>
                                    <tr className="name-ban-admin">
                                        <td>{selectedUser.userName}</td>
                                            <td><button disabled={selectedUser.id == currentuser} onClick={(e) => banUser(e, selectedUser)}>{selectedUser.isBanned ? "Unban" : "Ban"}</button></td>
                                            <td><button onClick={(e) => makeAdmin(e, selectedUser)}>{selectedUser.isAdmin ? "Revoke" : "Grant"}</button></td>
                                    </tr>
                                </tbody>
                                </table>
                           <div className="messagelabel">Messages</div>
                            <div className="userchats">
                                {chatContents}
                            </div>
                        </div>
                        <button onClick={backButton}>Back</button>
                    </>
                    )}
                </div>
            )}
        </div>
    );
}

export default AdminView;


