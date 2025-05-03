import React, { createContext, useState, useContext } from "react";
//import { jwtDecode } from "jwt-decode";

interface AuthContextType {
    loggedIn: boolean;
    login: (token: string) => void;
    logout: () => void;
}

export const AuthContext = createContext<AuthContextType>({
    loggedIn: false,
    login: () => { },
    logout: () => { },
});

// handles the login state so it can be referenced in other components
export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [loggedIn, setLoggedIn] = useState<boolean>(() => {
        const token = localStorage.getItem("token");
        return !!token;
    });

    const login = (token: string) => {
        localStorage.setItem("token", token);
        setLoggedIn(true);
    };

    const logout = () => {
        localStorage.removeItem("token");
        setLoggedIn(false);
    };



    return (
        <AuthContext.Provider value={{ loggedIn, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
