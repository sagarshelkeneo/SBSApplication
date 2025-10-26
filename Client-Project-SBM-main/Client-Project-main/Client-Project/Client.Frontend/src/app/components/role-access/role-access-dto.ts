export interface RoleAccessDto {
    u_roleName: string;
    a_screenName: string;
    a_screenCode: string;
    a_viewAccess: boolean;
    a_createAccess: boolean;
    a_editAccess: boolean;
    a_deleteAccess: boolean;
}

export interface RoleAccessByRoleIdDto {
    a_id: number;
    a_roleId: number;
    a_roleName: string;
    a_screenName: string;
    a_screenCode: string;
    a_viewAccess: boolean;
    a_createAccess: boolean;
    a_editAccess: boolean;
    a_deleteAccess: boolean;
}

export interface RoleAccessUpdateDto {
  id: number;
  viewAccess: boolean;
  createAccess: boolean;
  editAccess: boolean;
  deleteAccess: boolean;
  updatedBy: number;
}