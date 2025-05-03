import './App.css';

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Chat from './Pages/Chat.tsx';
import Favicon from "react-favicon";


function App() {
    const faviconUrl = 'img/favicon.ico';
    return (

        <BrowserRouter>
            <Favicon url={faviconUrl} />
            <Routes>
                <Route path="/" element={<Chat />} />
            </Routes>
        </BrowserRouter>
    );

}
export default App;