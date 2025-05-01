export interface LoginRequest {
    email: string,
    password: string
}

export interface RegisterRequest {
    username: string,
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    confirmPassword: string
}

export const login = async (loginRequest: LoginRequest) => {
    await fetch("https://localhost:7165/api/Auth/Login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(loginRequest)
    })
}

export const register = async (registerRequest: RegisterRequest) => {
    await fetch("https://localhost:7165/api/Auth/Register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(registerRequest)
    })
}