import mitt from "mitt"
import { Label } from "../services/labels"

type Events = {
    cardLabelsUpdated: { cardId: string; boardId: string; newLabels: Label[]; };
}

export const eventBus = mitt<Events>();