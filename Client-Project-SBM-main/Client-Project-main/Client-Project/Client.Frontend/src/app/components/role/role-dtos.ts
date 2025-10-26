export interface RoleGetDto {
    id: number;
    roleName: string;
    description: string;
}

export interface RoleCreateDto {
    roleName: string;
    description: string;
    createdBy: number;
}

export interface RoleUpdateDto {
    id: number;
    roleName: string;
    description: string;
    updatedBy: number;
}