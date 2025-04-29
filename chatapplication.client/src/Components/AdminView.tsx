import { useEffect, useState } from "react";

interface User {
    id: string;
    username: string;
    isAdmin: boolean;
    isBanned: boolean;
}

const AdminView: React.FC = () => {
    const [isOpen, setOpen] = useState(false);
    const [users, setUsers] = useState<User[]>([]);


    useEffect(() => {

        const fetchAvatars = async () => {
            try {
                const response = await fetch('/api/users', {
                    method: 'GET',
                });
                if (!response.ok) throw new Error('Failed to fetch users');
                const data: User[] = await response.json();
                setUsers(data);
            } catch (error) {
                console.error('Failed to get avatars:', error);
            }

        };

        fetchAvatars();

    }, []);

    const showMenu = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        setOpen(prev => !prev);
    };


    const contents = users === undefined
        ? <p><em>Loading...</em></p>
        :
        <ul >
            {users.map(user => {
                return (

                    <li key={user.id}></li>

                );
            })}
        </ul>;




  return (
      <div >
          <a href="#" onClick={showMenu}>
              Admin Tools
          </a>

          {isOpen && (
              <div >
              to do
                  {contents}
              </div>
          )}
      </div>
  );
}

export default AdminView;