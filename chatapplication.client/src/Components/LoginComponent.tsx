import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { useAuth } from "../Context/AuthContext";

function Login() {
    // state variables for email and passwords
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const { login } = useAuth();
    const [register, setRegister] = useState(false);
    // state variable for error messages
    const [error, setError] = useState<string>("");
    const navigate = useNavigate();

    const toggleRegister = () => setRegister(value => !value);
    // handle change events for input fields
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        if (name === "username") setUsername(value);
        if (name === "password") setPassword(value);
        if (register) {
            if (name === "confirmPassword") setConfirmPassword(value);
        }
    };


    // handle submit event for the form
    const handleLogin = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!username || !password) {
            setError("Please fill in all fields.");
            return;
        }

        setError("");

        fetch('/api/login', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                username: username,
                password: password,
            }),
        })
            .then(async (response) => {
                if (!response.ok) {
                    throw new Error("Login failed");
                }

                const data = await response.json(); // Parse the actual body from the response

                const token = data.token;
                localStorage.setItem("token", token);

                const decodedToken = jwtDecode<{ sub: string, userId: string }>(token);
                const loggedInUsername = decodedToken.sub;
                //const loggedInUserId = decodedToken.userId;

                console.log("Logged in as:", loggedInUsername);
                login(token);

                navigate("/");
            })
            .catch((error) => {
                console.error(error);
                setError("Error logging in.");
            });



    };

    const handleRegister = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        // validate email and passwords
        if (!username || !password || !confirmPassword) {
            setError("Please fill in all fields.");
        } else if (password !== confirmPassword) {
            setError("Passwords do not match.");
        } else {
            // clear error message
            setError("");
            // post data to the /register api
            fetch("/api/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    username: username,
                    password: password,
                }),
            })
                .then((data) => {
                    // handle success or error from the server
                    console.log(data);
                    if (data.ok) {
                        setError("Successful register.");
                        toggleRegister();
                    }
                    else
                        setError("Error registering.");
                

                })
                .catch((error) => {
                    // handle network error
                    console.error(error);
                    setError("Network Error.");
                });
        }
    };


    return (
        <div className="containerbox">
            {!register ? (
                <>
                    <h3>Login</h3>
                    <form onSubmit={handleLogin}>
                        <div>
                            <label className="forminput" htmlFor="username">Username:</label>
                        </div>
                        <div>
                            <input
                                type="text"
                                id="username"
                                name="username"
                                value={username}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <label htmlFor="password">Password:</label>
                        </div>
                        <div>
                            <input
                                type="password"
                                id="password"
                                name="password"
                                value={password}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <button type="submit">Login</button>
                        </div>

                    </form>
                    <div>
                        <button onClick={toggleRegister}>Register</button>
                    </div>
                </>
            ) : (
                <>
                    <h3>Register</h3>

                    <form onSubmit={handleRegister}>
                        <div>
                            <label htmlFor="username">Username:</label>
                        </div><div>
                            <input
                                type="text"
                                id="username"
                                name="username"
                                value={username}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <label htmlFor="password">Password:</label></div><div>
                            <input
                                type="password"
                                id="password"
                                name="password"
                                value={password}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <label htmlFor="confirmPassword">Confirm Password:</label></div><div>
                            <input
                                type="password"
                                id="confirmPassword"
                                name="confirmPassword"
                                value={confirmPassword}
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <button type="submit">Register</button>

                        </div>

                    </form>
                    <div>
                        <button onClick={toggleRegister}>Go to Login</button>
                    </div>
                </>)}

            {error && <p className="error">{error}</p>}
        </div>
    );
}

export default Login;
