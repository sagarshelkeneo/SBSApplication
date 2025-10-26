export interface BankGetDto {
    r_id: number;
    r_bankName: string;
    r_branch: string;
}

export interface BankUpdateDto {
    id: number;
    bankName: string;
    branch: string;
    updatedBy: number;
}

export interface BankCreateDto {
    bankName: string;
    branch: string;
    createdBy: number;
}