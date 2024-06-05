const isKubernetes = process.env.NEXT_PUBLIC_KUBERNETES === 'true';

export const API_URL_POSTS = isKubernetes 
  ? `${process.env.NEXT_PUBLIC_API_URL_KUBERNETES}/posts`
  : process.env.NEXT_PUBLIC_API_URL_POSTS;

export const API_URL_LIKE = isKubernetes 
  ? `${process.env.NEXT_PUBLIC_API_URL_KUBERNETES}/l/likes`
  : process.env.NEXT_PUBLIC_API_URL_LIKE;

export const API_URL_USER = isKubernetes
    ? `${process.env.NEXT_PUBLIC_API_URL_KUBERNETES}/user`
    : process.env.NEXT_PUBLIC_API_URL_USER;

export const AUTH_EMAIL = process.env.AUTH_EMAIL;
export const AUTH_PASSWORD = process.env.AUTH_PASSWORD;