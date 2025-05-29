import {Badge, Box, HStack, Text} from "@chakra-ui/react";
import { Card } from "../../services/cards";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import Link from "next/link"
import {getContrastTextColor} from "@/app/utils/colorUtils";

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
    
    return (
        <Link href={`/c/${card.id}`} passHref>
            <Box
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
            minH="50px"
            bg="gray.100"
            borderRadius="md"
            opacity={isDragging ? 0.7 : 1}
            p={2}
            _hover={{bg: "gray.200"}}
            boxShadow="sm"
            cursor="pointer"
            mb={1}
            >
                {card.labels && card.labels.length > 0 && (
                    <HStack spacing={1} mb={2} wrap="wrap">
                        {card.labels.map((label) => (
                            <Badge
                                key={label.id}
                                bg={label.color}
                                color={getContrastTextColor(label.color)}
                                px={2}
                                py={0.5}
                                borderRadius="sm"
                                fontSize="xs"
                            >
                                {label.name}
                            </Badge>
                        ))}
                    </HStack>
                )}
                <Text fontSize="sm" fontWeight="medium" color="gray.800">
                    {card.title}
                </Text>
            </Box>
        </Link>
  );
}