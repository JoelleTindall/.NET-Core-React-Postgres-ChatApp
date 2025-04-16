
import React, { useEffect, useState } from 'react';
import { AuthorizedUser } from './AuthorizeView';
import '../assets/styles/AvatarPickerStyle.css';
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


const AvatarPicker: React.FC = () => {
    const [avatars, setAvatars] = useState<Avatar[]>([]);
    const [isOpen, setOpen] = useState(false);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);

    useEffect(() => {

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

        const fetchAvatars = async () => {
            try {
                const response = await fetch('/avatar');
                if (!response.ok) throw new Error('Failed to fetch avatars');
                const data: Avatar[] = await response.json();
                setAvatars(data);
            } catch (error) {
                console.error('Failed to get avatars:', error);
            }

        };

        fetchAvatars();

    }, []);

    const handleSetAvatar = async (avatarId: string) => {
        try {
            const response = await fetch('/avatar/set', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(avatarId)
            });

            if (!response.ok) {
                throw new Error('Failed to set avatar');
            }

            setOpen(false); // close the menu
        } catch (error) {
            console.error(error);
            alert('Something went wrong.');
        }
    };

    const contents = avatars === undefined
        ? <p><em>Loading...</em></p>
        :
        <ul className="avatar-list">
            {avatars.map(avatar => {
                return (
                    
                    <li key={avatar.id} className="avatar-list-item" onClick={() => handleSetAvatar(avatar.id)} ><img src={`/${avatar.filePath}`} /></li>
                    
                );
            })}
        </ul>;

    //onClick={handleSetAvatar(avatar.id)}
    const showMenu = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        setOpen(prev => !prev);
    };

    return (
        <div className="avatar-picker">
            <a href="#" onClick={showMenu}>
                Choose Avatar
            </a>

            {isOpen && (
                <div className="avatar-dropdown">
                    {contents}
                </div>
            )}
        </div>
    );
}

export default AvatarPicker;