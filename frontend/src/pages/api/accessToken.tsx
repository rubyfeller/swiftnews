import { NextApiRequest, NextApiResponse } from 'next';
import { getAccessToken } from '@auth0/nextjs-auth0';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  try {
    const accessToken = await getAccessToken(req, res);
    res.status(200).json({ accessToken });
  } catch (error) {
    console.error('Error fetching access token:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
}
