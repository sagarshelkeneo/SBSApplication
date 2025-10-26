export interface Login {
    username: string;
    password: string;
    recaptchaToken: string;
}

export interface AuthResponse {
    token: string;
}

export interface JwtClaims {
    user: string;
    userId: number;
    role: string;
    CompanyID: number;
    Email: string;
}
