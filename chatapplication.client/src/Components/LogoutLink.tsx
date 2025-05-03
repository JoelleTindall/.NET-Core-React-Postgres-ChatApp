import { useAuth } from "../Context/AuthContext";

function LogoutLink(props: { children: React.ReactNode }) {

    const { logout } = useAuth();

    const handleLogout = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        logout();


    };

    return (
        <>
            <a href="#" onClick={handleLogout}>{props.children}</a>
        </>
    );
}

export default LogoutLink;
