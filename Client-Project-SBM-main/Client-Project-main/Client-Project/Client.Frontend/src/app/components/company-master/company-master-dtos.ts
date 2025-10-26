export interface CompanyMasterCreateDto {
    name: string;
    address: string
    phone: string
    email: string
    createdBy: number
}

export interface CompanyMasterGetDto {
    id :number;
    name : string;
    address : string;
    phone : string;
    email :string;
}

export interface CompanyMasterUpdateDto {
    id: number;
    name: string;
    address: string;
    phone: string;
    email: string;
    updatedBy: number;
}