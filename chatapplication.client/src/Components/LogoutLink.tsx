
//import { useNavigate } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";

function LogoutLink(props: { children: React.ReactNode }) {

    //const navigate = useNavigate();
    const { logout } = useAuth();

    const handleLogout = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        logout();
        //localStorage.removeItem("token");
        //navigate("/login");

    };

    return (
        <>
            <a href="#" onClick={handleLogout}>{props.children}</a>
        </>
    );
}

export default LogoutLink;
