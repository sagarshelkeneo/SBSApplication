export interface ProductGetDto {
    r_id: number;
    r_description: string;
    r_unitPrice: number;
    r_companyID: number;
}

export interface ProductCreateDto {
    description: string;
    unitPrice: number;
    companyId: number;
    createdBy: number;
}

export interface ProductUpdateDto {
    id: number;
    companyId: number;
    description: string;
    unitPrice: number;
    updatedBy: number;
}
