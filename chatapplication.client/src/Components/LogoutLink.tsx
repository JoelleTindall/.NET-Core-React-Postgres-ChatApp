
import { useNavigate } from "react-router-dom";

function LogoutLink(props: { children: React.ReactNode }) {

    const navigate = useNavigate();


    const handleLogout = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();

        localStorage.removeItem("token");
        navigate("/login");

    };

    return (
        <>
            <a href="#" onClick={handleLogout}>{props.children}</a>
        </>
    );
}

export default LogoutLink;
