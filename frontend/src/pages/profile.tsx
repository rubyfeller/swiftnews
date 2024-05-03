import { useUser } from '@auth0/nextjs-auth0/client';

export default function ProfileClient() {
    const { user, error, isLoading } = useUser();

    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>{error.message}</div>;

    return (
        user && (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-center">
                    <img src={user.picture ?? ''} alt={user.name ?? ''} className="mx-auto mb-4 rounded-full" />
                    <p className="font-bold">Username</p>
                    <h2 className="mb-4">{user.name}</h2>
                    <p className="font-bold">Last updated at</p>
                    <p className="mb-4">{user.updated_at}</p>
                    <a href="../" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full mr-2">
                        Go back
                    </a>
                </div>
            </div>
        )
    );
}
