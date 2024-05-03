import { NextApiRequest, NextApiResponse } from 'next';

const messages = [
    {
        "content": "Global financial crisis triggers stock market crash",
        "author": "@reuters"
    },
    {
        "content": "Apple unveils new iPhone 13",
        "author": "@techCrunch"
    },
    {
        "content": "Tesla stock plummets after Elon Musk tweets",
        "author": "@bloomberg"
    },
    {
        "content": "Facebook rebrands to Meta",
        "author": "@theverge"
    },
    {
        "content": "Google announces new Pixel 6",
        "author": "@9to5Google"
    }
];

export default function handler(req: NextApiRequest, res: NextApiResponse) {
    res.status(200).json(messages);
}
