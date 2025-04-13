//import WeatherForecast from "../Components/WeatherForecast.tsx";
import ChatComponent from "../Components/ChatComponent.tsx";
import LogoutLink from "../Components/LogoutLink.tsx";
import AuthorizeView, { AuthorizedUser } from "../Components/AuthorizeView.tsx";

function Home() {
    return (
        <AuthorizeView>
            <span><LogoutLink>Logout <AuthorizedUser value="email" /></LogoutLink></span>
            <ChatComponent />
        </AuthorizeView>
    );
}

export default Home;