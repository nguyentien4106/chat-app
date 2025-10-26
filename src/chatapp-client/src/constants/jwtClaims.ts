// ==========================================
// src/constants/jwtClaims.ts
// ==========================================
// .NET ClaimTypes URIs for JWT token claims
export const JWT_CLAIMS = {
  NAME_IDENTIFIER: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  EMAIL: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  NAME: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  ROLE: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
} as const;
