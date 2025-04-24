import ChatComponent from "../Components/ChatComponent.tsx";
import Login from "../Components/LoginComponent.tsx";
import { useAuth } from "../Context/AuthContext";


function Chat() {
    const { loggedIn } = useAuth();
    
        if (loggedIn)
        {
            return <ChatComponent/>;
        } else {
            return <Login />;
        }

  
   
}

export default Chat;