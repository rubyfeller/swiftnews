interface Message {
    content: string;
    author: string;
}

const MessageItem: React.FC<Message> = ({ content, author }) => {
    return (
        <div>
            <p className="font-semibold">{author}</p>
            <p>{content}</p>
        </div>
    );
};

export default MessageItem;
