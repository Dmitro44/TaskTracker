import { AddIcon } from "@chakra-ui/icons";
import { Box, VStack, Button, Input, Text } from "@chakra-ui/react";
import { Column } from "../../services/columns";
import { CardBox } from "./CardBox";
import { SortableContext, useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { useMemo, useState } from "react";
import { Card } from "../../services/cards";

interface ColumnBoxProps {
    column: Column;
    cards: Card[];
    onAddCard: (columnId: string) => void;
    updateColumn: (columnId: string, title: string) => void;
}

export function ColumnBox({ column, cards, onAddCard, updateColumn }: ColumnBoxProps) {

    const [editMode, setEditMode] = useState(false);
    
    const cardIds = useMemo(() => {
        return cards.map((card) => card.id)
    }, [cards])

    const { setNodeRef, attributes, listeners, transform, transition, isDragging } 
    = useSortable({
        id: column.id,
        data: {
            type: "Column",
            column,
        },
        disabled: editMode,
    });

    const style = {
        transition,
        transform: CSS.Translate.toString(transform),
    }

    if (isDragging) {
        return (
            <Box
                ref={setNodeRef}
                style={style}
                w="250px"
                minH="200px"
                bg="white"
                borderRadius="md"
                boxShadow="md"
                p={4}
                display="flex"
                flexDirection="column"
                userSelect="none"
                opacity={0.7}
            />
        );
    }

    return (
        <Box
            ref={setNodeRef}
            style={style}
            w="250px"
            minH="200px"
            bg="white"
            borderRadius="md"
            boxShadow="md"
            p={4}
            display="flex"
            flexDirection="column"
            userSelect="none"
        >
            <Text
                as="b"
                mb={2}
                {...attributes}
                {...listeners}
                onClick={() => {setEditMode(true);}}
            >
                {!editMode && column.title}
                {editMode && 
                    <Input
                        value={column.title}
                        onChange={(e) => updateColumn(column.id, e.target.value)}
                        autoFocus
                        onBlur={() => {
                            setEditMode(false);
                        }}
                        onKeyDown={(e) => {
                            if (e.key !== "Enter") return;
                            setEditMode(false);
                        }}
                    />
                }
            </Text>
            <SortableContext items={cardIds}>
                <VStack spacing={2} align="stretch" flex="1" overflowY="auto">
                    {cards.map((card) => (
                        <CardBox
                            key={card.id}
                            card={card}
                        />
                    ))}
                </VStack>
            </SortableContext>
            <Button
                size="sm"
                mt={2}
                variant="ghost"
                colorScheme="blue"
                opacity={0.8}
                leftIcon={<AddIcon boxSize={3} />}
                onClick={() => onAddCard(column.id)}
            >
                Добавить карточку
            </Button>
        </Box>
      );
}