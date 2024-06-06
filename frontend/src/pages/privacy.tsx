import Head from 'next/head';
import Link from 'next/link';

const PrivacyPolicyPage: React.FC = () => {
    return (
        <div className="min-h-screen flex flex-col justify-center items-center">
            <Head>
                <title>Privacy Policy | SwiftNews</title>
                <meta name="description" content="Privacy policy for SwiftNews" />
                <link rel="icon" href="/favicon.ico" />
            </Head>

            <div className="max-w-3xl mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold mb-6">Privacy Policy</h1>

                <p className="mb-4">
                    Last updated: June 5, 2024.
                </p>

                <h2 className="text-xl font-bold mb-4">Introduction</h2>
                <p className="mb-6">
                    At SwiftNews, we are committed to protecting your privacy. This Privacy Policy outlines how we collect, use, disclose, and protect your personal information when you use our website and services.
                </p>

                <h2 className="text-xl font-bold mb-4">Information We Collect</h2>
                <p className="mb-6">
                    We collect various types of information when you use SwiftNews, including:
                </p>
                <ul className="list-disc ml-6 mb-6">
                    <li>User authentication data provided by Auth0</li>
                    <li>Posts created by users</li>
                    <li>User IDs and usernames</li>
                    <li>Likes given by users</li>
                </ul>

                <h2 className="text-xl font-bold mb-4">How We Use Your Information</h2>
                <p className="mb-6">
                    We use the information we collect for various purposes, including:
                </p>
                <ul className="list-disc ml-6 mb-6">
                    <li>Providing and maintaining our services</li>
                    <li>Personalizing user experience</li>
                    <li>Analyzing usage trends and improving our services</li>
                    <li>Responding to user inquiries and support requests</li>
                </ul>

                <h2 className="text-xl font-bold mb-4">Data Retention and Deletion</h2>
                <p className="mb-6">
                    Users have the right to be forgotten. You can request the deletion of your account and all associated data by pressing the delete button. For insights into your data, please contact support at support@swiftnews.com.
                </p>
            </div>
            <Link href="/">
                <button className="text-blue-500 hover:underline focus:outline-none">Go back</button>
            </Link>
        </div>
    );
};

export default PrivacyPolicyPage;
