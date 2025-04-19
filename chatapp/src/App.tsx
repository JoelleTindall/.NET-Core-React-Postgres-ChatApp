import './App.css';

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Register from './Pages/Register.tsx';

function App() {
    return (
        <BrowserRouter>
            <Routes>

                <Route path="/" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );

}
export default App;