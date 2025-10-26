export interface SubContractorGetDto {
    id: number;
    companyId: number;
    name: string;
}

export interface SubContractorCreateDto {
    companyId: number;
    name: string;
    createdBy: string;
}

export interface SubContractorUpdateDto {
    id: number;
    companyId: number;
    name: string;
    updatedBy: number;
}