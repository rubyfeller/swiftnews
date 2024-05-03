import { AppProps } from "next/app";
import RootLayout from "../app/layout";

function SwiftNews({ Component, pageProps }: AppProps) {
  return (
    <RootLayout>
      <Component {...pageProps} />
    </RootLayout>
  );
}

export default SwiftNews;
