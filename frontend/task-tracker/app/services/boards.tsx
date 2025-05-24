import { apiFetch } from "./apiFetch";
import { ColumnFull } from "./columns";

export type Board = {
    id: string;
    title: string;
    isPublic: boolean; 
};

export type BoardFull = {
    id: string;
    title: string;
    isPublic: boolean;
    columns: ColumnFull[];
}


export async function getBoards(): Promise<Board[]> {
    const response = await apiFetch("https://localhost:7165/api/Board/getBoards", {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }

    const data = await response.json();
    return data.boards;
}

export async function createBoard({ title, isPublic } : { title: string, isPublic: boolean }) {
    const res = await apiFetch("https://localhost:7165/api/Board/create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ title, isPublic }),
    });
    
    if (!res.ok) {
        throw new Error("Error creating board");
    }
}

export async function getFullBoard(boardId: string) : Promise<BoardFull> {
    const res = await apiFetch(`https://localhost:7165/api/Board/${boardId}/getBoard`, {
        credentials: "include",
    })

    return res.json();
}