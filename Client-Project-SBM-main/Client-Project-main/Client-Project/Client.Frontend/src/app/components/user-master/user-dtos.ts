export interface UserGetDto {
    id: number;
    companyID: number;
    roleMasterId: number;
    roleName: string;
    username: string;
    email: string;
    isActive: number;
    password: string;
}

export interface UserCreateDto {
    roleMasterId: number;
    companyID: number;
    username: string;
    email: string;
    password: string;
    isActive: number;
    createdBy: number;
}

export interface UserUpdateDto {
    id: number;
    companyID: number;
    roleMasterId: number;
    username: string;
    email: string;
    isActive: number;
    updatedBy: number;
}

export interface ChangePasswordDto {
    username: string;
    email: string;
    currentPassword: string;
    newPassword: string;
}