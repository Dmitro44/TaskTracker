import { Box, Text } from "@chakra-ui/react";
import { Card } from "../../services/cards";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

export function CardBox({ card } : { card: Card }) {

    const { setNodeRef, attributes, listeners, transform, transition, isDragging } 
    = useSortable({
        id: card.id,
        data: {
            type: "Card",
            card,
        },
    });

    const style = {
        transition,
        transform: CSS.Transform.toString(transform),
    }

    if (isDragging) {
        return (
            <Box
                ref={setNodeRef}
                style={style}
                {...attributes}
                {...listeners}
                minH="50px"
                opacity={0.7}
                bg="gray.100"
                borderRadius="md"
                p={2}
                _hover={{ bg: "gray.200" }}
                boxShadow="sm"
                cursor="pointer"
                mb={1}
            />
        );
    }
    
    return (
        <Box
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
            minH="50px"
            bg="gray.100"
            borderRadius="md"
            p={2}
            _hover={{ bg: "gray.200" }}
            boxShadow="sm"
            cursor="pointer"
            mb={1}
        >
            <Text fontSize="sm">{card.title}</Text>
        </Box>
  );
}