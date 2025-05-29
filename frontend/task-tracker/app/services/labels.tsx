import {apiFetch} from "@/app/services/apiFetch";

export type Label = {
    id: string;
    name: string;
    color: string;
    boardId: string;
}

export async function getLabelsForCard( cardId: string): Promise<Label[]> {
    const response = await apiFetch(`https://localhost:7165/api/Label/${cardId}/getLabelsForCard`, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }

    return response.json();
}

export async function getLabelsForBoard( boardId: string): Promise<Label[]> {
    const response = await apiFetch(`https://localhost:7165/api/Label/${boardId}/getLabels`, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }

    return response.json();
}

export async function createLabel( boardId: string, name: string, color: string) {
    const res = await apiFetch("https://localhost:7165/api/Label/create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ name, color, boardId }),
    });

    if (!res.ok) {
        throw new Error(`Error: ${res.statusText}`);
    }
}