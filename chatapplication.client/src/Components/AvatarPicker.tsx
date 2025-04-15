
import React, { useEffect, useState } from 'react';
import { AuthorizedUser } from './AuthorizeView';
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


    return (
        //<link href="#" toggle={isOpen}>
      <div>
        <ul>
           {avatars.map(avatar => {
           return (
             
                <li key={avatar.id} ><img src={`/${avatar.filePath}`} /></li>

            );
        })}
               </ul>
            </div>
        //</link>
  );
}

export default AvatarPicker;