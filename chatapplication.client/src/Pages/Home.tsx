//import WeatherForecast from "../Components/WeatherForecast.tsx";
import ChatComponent from "../Components/ChatComponent.tsx";
import AuthorizeView from "../Components/AuthorizeView.tsx";

function Home() {
    return (
        <AuthorizeView>
   
            <ChatComponent />
        </AuthorizeView>
    );
}

export default Home;