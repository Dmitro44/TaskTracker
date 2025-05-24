import { apiFetch } from "./apiFetch";
import { Card } from "./cards";

export type Column = {
    id: string;
    title: string;
    position: number;
    boardId: string;
}

export type ColumnFull = {
    id: string;
    title: string;
    position: number;
    boardId: string;
    cards: Card[]
}

export async function getColumns(boardId: string): Promise<Column[]> {
    const response = await apiFetch(`https://localhost:7165/api/Column/getColumns?boardId=${boardId}`, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }

    const data = await response.json();
    return data.columns;
}

export async function createColumn({ title, position, boardId } : { title: string, position: number, boardId: string}): Promise<ColumnFull> {
    const res = await apiFetch("https://localhost:7165/api/Column/create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ title, position, boardId }),
    });

    if (!res.ok) {
        throw new Error("Error creating column");
    }

    return res.json();
}

export async function moveColumn({ columnId, position } : {columnId: string, position: number}){
    const res = await apiFetch(`https://localhost:7165/api/Column/${columnId}/move`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(position),
    });

    const data = await res.json();

    if (!res.ok && data.position !== position) {
        throw new Error(`Error moving column ${columnId} to position ${position}`);
    }
}

export async function updateColumn({ columnId, title }: {columnId: string, title: string}) {
    const res = await apiFetch(`https://localhost:7165/api/Column/${columnId}/update`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ title }),
    });

    if (!res.ok) {
        throw new Error(`Error updating column ${columnId}`);
    }
}