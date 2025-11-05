import { PaginationRequest } from "@/types"

export const UserPrivileges = {
    0: "Normal User",
    1: "Inactive User",
    2: "Enroller",
    14: "Admin"
}

export const defaultPaginationRequest: PaginationRequest = {
    pageNumber: 1,
    pageSize: 50,
    sortOrder: 'asc',
};