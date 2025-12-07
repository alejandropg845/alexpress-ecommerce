export interface LoginResponse {
    accessToken: string,
    refreshToken: string,
    ok: boolean,
    isTwoFactorEnabled: boolean,
    partialToken: string
}