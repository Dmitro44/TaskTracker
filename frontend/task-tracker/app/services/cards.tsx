import { apiFetch } from "./apiFetch";

export type Card = {
    id: string;
    title: string;
    position: number;
    columnId: string;
}

export async function createCard({ title, position, columnId} : {title: string, position: number, columnId: string}): Promise<Card> {
    const res = await apiFetch("https://localhost:7165/api/Card/create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ title, position, columnId }),
    });

    if (!res.ok) {
        throw new Error("Error creating card");
    }

    return res.json();
}

export async function moveCard({ cardId, toColumnId, position} : {cardId: string, toColumnId: string, position: number}) {
    const res = await apiFetch(`https://localhost:7165/api/Card/${cardId}/move`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ position, toColumnId }),
    });

    if (!res.ok) {
        throw new Error(`Error moving card ${cardId} to column ${toColumnId} to position ${position}`);
    }
}