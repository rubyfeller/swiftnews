"use client";
import { useUser } from "@auth0/nextjs-auth0/client";

function UserComponent() {
  const { user, error, isLoading } = useUser();

  if (isLoading) return <div>Loading...</div>;

  return (
    <main className="flex min-h-screen items-center justify-center">
      <div className="text-center">
        <h1 className="mb-4 text-4xl font-extrabold leading-none tracking-tight text-gray-900 md:text-5xl lg:text-6xl dark:text-white">SwiftNews</h1>
        {error && <div>{error.message}</div>}
        {user ? (
          <><a href="/api/auth/logout" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full mr-2">
            Logout
          </a><a href="/profile" className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded-full mr-2">
              Profile
            </a><a href="/latest" className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded-full mr-2">
              Latest messages
            </a></>

        ) : (
          <div>
            <p className="mb-6 text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400">Login or register to enjoy our beautiful platform.</p>
            <a href="/api/auth/login" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full mr-2" data-testid='login-button'>
              Login
            </a>
            <a href="/api/auth/login" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full">
              Register
            </a>
          </div>
        )}
      </div>
    </main>
  );
}

export default function Home() {
  return (
    <UserComponent />
  );
}