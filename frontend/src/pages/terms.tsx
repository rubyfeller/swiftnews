import Head from 'next/head';
import Link from 'next/link';

const TermsAndConditionsPage: React.FC = () => {
    return (
        <div className="min-h-screen flex flex-col justify-center items-center">
            <Head>
                <title>Terms and Conditions | SwiftNews</title>
                <meta name="description" content="Terms and conditions for SwiftNews" />
                <link rel="icon" href="/favicon.ico" />
            </Head>

            <div className="max-w-3xl mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold mb-6">Terms and Conditions</h1>

                <p className="mb-4">
                    Last updated: June 5, 2024.
                </p>

                <h2 className="text-xl font-bold mb-4">Introduction</h2>
                <p className="mb-6">
                    Welcome to SwiftNews. By accessing or using our website and services, you agree to comply with and be bound by these Terms and Conditions. Please read them carefully.
                </p>

                <h2 className="text-xl font-bold mb-4">1. Acceptance of Terms</h2>
                <p className="mb-6">
                    By using SwiftNews, you accept and agree to be bound by these Terms and Conditions and our Privacy Policy. If you do not agree to these terms, you should not use our services.
                </p>

                <h2 className="text-xl font-bold mb-4">2. Use of Services</h2>
                <ul className="list-disc ml-6 mb-6">
                    <li className="mb-4"><strong>Eligibility:</strong> You must be at least 18 years old to use our services.</li>
                    <li className="mb-4"><strong>Account Responsibility:</strong> You are responsible for maintaining the confidentiality of your account and password and for all activities that occur under your account.</li>
                    <li className="mb-4"><strong>Prohibited Activities:</strong> You agree not to use our services for any unlawful or prohibited activities, including but not limited to spamming, harassment, and infringement of intellectual property rights.</li>
                </ul>

                <h2 className="text-xl font-bold mb-4">3. User Content</h2>
                <ul className="list-disc ml-6 mb-6">
                    <li className="mb-4"><strong>Ownership:</strong> You retain ownership of any content you post on SwiftNews. By posting content, you grant us a non-exclusive, royalty-free, worldwide license to use, display, and distribute your content.</li>
                    <li className="mb-4"><strong>Prohibited Content:</strong> You agree not to post any content that is illegal, harmful, threatening, abusive, defamatory, or otherwise objectionable.</li>
                </ul>

                <h2 className="text-xl font-bold mb-4">4. Termination</h2>
                <p className="mb-6">
                    We reserve the right to suspend or terminate your account and access to our services at our sole discretion, without notice, for conduct that we believe violates these Terms and Conditions or is harmful to other users.
                </p>

                <h2 className="text-xl font-bold mb-4">5. Limitation of Liability</h2>
                <p className="mb-6">
                    SwiftNews will not be liable for any direct, indirect, incidental, consequential, or punitive damages arising out of your use of our services.
                </p>

                <h2 className="text-xl font-bold mb-4">6. Indemnification</h2>
                <p className="mb-6">
                    You agree to indemnify and hold harmless SwiftNews, its affiliates, and their respective officers, directors, employees, and agents from any claims, liabilities, damages, losses, or expenses arising from your use of our services or your violation of these Terms and Conditions.
                </p>

                <h2 className="text-xl font-bold mb-4">7. Changes to Terms</h2>
                <p className="mb-6">
                    We may update these Terms and Conditions from time to time. Any changes will be posted on this page with an updated revision date. Your continued use of our services after any changes indicates your acceptance of the new Terms and Conditions.
                </p>

                <h2 className="text-xl font-bold mb-4">8. Governing Law</h2>
                <p className="mb-6">
                    These Terms and Conditions are governed by and construed in accordance with the laws of The Netherlands, without regard to its conflict of law principles.
                </p>

                <h2 className="text-xl font-bold mb-4">Contact Us</h2>
                <p className="mb-6">
                    If you have any questions about these Terms and Conditions, please contact us at <a href="mailto:support@swiftnews.com" className="text-blue-500 hover:underline">support@swiftnews.com</a>.
                </p>
            </div>
            <Link href="/">
                <button className="text-blue-500 hover:underline focus:outline-none mb-8">Go back</button>
            </Link>
        </div>
    );
};

export default TermsAndConditionsPage;
