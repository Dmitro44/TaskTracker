import CardModalPage from "@/app/c/[cardId]/page";
import React from "react";

export default function InterceptedCardModal({ params } : { params: Promise<{ boardId: string, cardId: string }> }) {
    const { cardId, boardId } = React.use(params);

    return <CardModalPage cardId={cardId} boardId={boardId} />
}