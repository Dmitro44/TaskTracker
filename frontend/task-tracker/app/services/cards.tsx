import { apiFetch } from "./apiFetch";
import {Label} from "@/app/services/labels";

export type Card = {
    id: string;
    title: string;
    position: number;
    columnId: string;
    labels: Label[]
}

export type CardFull = {
    id: string;
    title: string;
    labels: Label[];
    // comments: ...
}

export async function createCard(title: string, position: number, columnId: string): Promise<Card> {
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

export async function moveCard(cardId: string, toColumnId: string, position: number) {
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

export async function addLabel(cardId: string, labelId: string): Promise<void> {
    const res = await apiFetch(`https://localhost:7165/api/Card/${cardId}/addLabel`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(labelId),
    });
    
    if (!res.ok) {
        throw new Error(`Error adding label ${labelId} to card ${cardId}`);
    }
}

export async function removeLabel( cardId: string, labelId: string): Promise<void> {
    const res = await apiFetch(`https://localhost:7165/api/Card/${cardId}/removeLabel/${labelId}`, {
        method: "DELETE",
        credentials: "include",
    });
    
    if (!res.ok) {
        throw new Error(`Error removing label ${labelId} from card ${cardId}`);
    }
}