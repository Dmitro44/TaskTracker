"use client"

import {
    Avatar,
    Badge,
    Box, Button, Divider, Flex, Grid, GridItem,
    Heading, HStack, IconButton,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent,
    ModalHeader,
    ModalOverlay, Spacer,
    Text, Textarea, VStack
} from "@chakra-ui/react";
import { useRouter } from "next/navigation";
import React, {useEffect, useState} from "react";
import {getLabelsForBoard, getLabelsForCard, Label} from "../../services/labels"
import {
    AddIcon,
    AttachmentIcon,
    CalendarIcon,
    ChatIcon,
    EditIcon,
    TimeIcon,
    ViewIcon
} from "@chakra-ui/icons";
import ManageLabelsModal from "@/app/components/card/ManageLabelsModal";
import {getContrastTextColor} from "@/app/utils/colorUtils";
import {eventBus} from "@/app/utils/eventBus";

interface CardModalPageProps {
    cardId?: string;    // Приходит как прямой пропс от InterceptedCardModal
    boardId?: string;   // Приходит как прямой пропс от InterceptedCardModal
    params?: Promise<{cardId: string}>;
}

export default function CardModalPage( props: CardModalPageProps) {
    const router = useRouter();

    let resolvedCardIdFromParams: string | undefined;

    if (!props.cardId && props.params) {
        const pageParamsObject = React.use(props.params);
        resolvedCardIdFromParams = pageParamsObject.cardId;
    }

    const cardId = props.cardId || resolvedCardIdFromParams;
    const boardId = props.boardId;

    const [isManageLabelsModalOpen, setIsManageLabelsModalOpen] = useState(false);

    const [cardLabels, setCardLabels] = useState<Label[]>([]);
    const [allBoardLabels, setAllBoardLabels] = useState<Label[]>([]);

    const fetchCard = async () => {
        if (!cardId){
            console.warn("Card ID is not available, skipping data fetch.");
            return;
        }

        try {
            console.log(`Fetching data for card: ${cardId}, board: ${boardId}`);
            const fetchedCardLabels = await getLabelsForCard(cardId);
            setCardLabels(fetchedCardLabels);

            if (boardId) {
                const fetchedAllBoardLabels = await getLabelsForBoard(boardId);
                setAllBoardLabels(fetchedAllBoardLabels);
            } else {
                console.warn("Board ID is not available. Board-specific labels might not be loaded.");
                setAllBoardLabels([]);
            }
            
            return fetchedCardLabels;
        } catch (error) {
            console.error("Failed to fetch card data: ", error);
        }
    };

    useEffect(() => {
        fetchCard();
    }, [cardId, boardId]);

    const [description, setDescription] = useState("Это описание карточки, которое можно редактировать. Здесь может быть подробная информация о задаче.");
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [newComment, setNewComment] = useState("");

    const handleClose = () => {
        router.back();
    }

    const handleLabelsUpdatedFromModal = async () => {
        const newCardLabels = await fetchCard();
        
        if (cardId && boardId && newCardLabels) {
            eventBus.emit(`cardLabelsUpdated`, { 
                cardId: cardId,
                boardId: boardId,
                newLabels: newCardLabels,
            });
        } else {
            console.warn("CardModalPage: Cannot emit 'cardLabelsUpdated' event, cardId or boardId is missing.");
        }
    }

    const mockCard = {
        id: "card-1",
        title: "Разработать новую функциональность",
        description: description,
        labels: [
            { id: 1, name: "Важно", color: "red" },
            { id: 2, name: "Frontend", color: "blue" },
            { id: 3, name: "В работе", color: "yellow" }
        ],
        members: [
            { id: 1, name: "Анна Иванова", avatar: "/placeholder.svg", initials: "АИ" },
            { id: 2, name: "Петр Петров", avatar: "/placeholder.svg", initials: "ПП" }
        ],
        dueDate: "2024-12-30",
        comments: [
            {
                id: 1,
                author: "Анна Иванова",
                avatar: "/placeholder.svg",
                initials: "АИ",
                text: "Начала работу над этой задачей. Планирую закончить к концу недели.",
                date: "2 часа назад"
            },
            {
                id: 2,
                author: "Петр Петров",
                avatar: "/placeholder.svg",
                initials: "ПП",
                text: "Отлично! Если нужна помощь с тестированием, дай знать.",
                date: "1 час назад"
            }
        ],
        attachments: [
            { id: 1, name: "design-mockup.figma", size: "2.4 MB" },
            { id: 2, name: "requirements.pdf", size: "1.1 MB" }
        ]
    };

    return (
        <>
            <Modal isOpen={true} onClose={handleClose} size="6xl" scrollBehavior="inside">
                <ModalOverlay/>
                <ModalContent maxH="90vh">
                    <ModalHeader borderBottom="1px solid" borderColor="gray.200" pb={4}>
                        <HStack spacing={3}>
                            <Box p={2} bg="gray.100" borderRadius="lg">
                                <EditIcon w={5} h={5} color="gray.600"/>
                            </Box>
                            <VStack align="start" spacing={1}>
                                <Heading size="lg" color="gray.900">
                                    {mockCard.title}
                                </Heading>
                                <Text fontSize="sm" color="gray.500">
                                    в списке <Text as="span" fontWeight="medium">Запланировано</Text>
                                </Text>
                            </VStack>
                            <Spacer/>
                        </HStack>
                    </ModalHeader>

                    <ModalCloseButton/>

                    <ModalBody p={6}>
                        <Grid templateColumns="3fr 1fr" gap={6}>
                            {/* Левая колонка - основной контент */}
                            <GridItem>
                                <VStack spacing={6} align="stretch">
                                    {/* Метки и участники */}
                                    <VStack spacing={4} align="stretch">
                                        {cardLabels && cardLabels.length > 0 && (
                                            <VStack align="stretch" spacing={2}>
                                                <HStack>
                                                    <EditIcon w={4} h={4}/>
                                                    <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                        Метки
                                                    </Text>
                                                </HStack>
                                                <HStack spacing={2}>
                                                    {cardLabels.map((label) => (
                                                        <Badge key={label.id} bg={label.color} color={getContrastTextColor(label.color)} fontSize="lg">
                                                            {label.name}
                                                        </Badge>
                                                    ))}
                                                    <IconButton
                                                        variant="outline"
                                                        size="sm"
                                                        icon={<AddIcon/>}
                                                        aria-label="Добавить метку"
                                                        onClick={() => setIsManageLabelsModalOpen(true)}
                                                    />
                                                </HStack>
                                            </VStack>
                                        )}

                                        {mockCard.members.length > 0 && (
                                            <VStack align="stretch" spacing={2}>
                                                <HStack>
                                                    <EditIcon w={4} h={4}/>
                                                    <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                        Участники
                                                    </Text>
                                                </HStack>
                                                <HStack spacing={2}>
                                                    {mockCard.members.map((member) => (
                                                        <HStack key={member.id} spacing={2}>
                                                            <Avatar size="sm" name={member.initials} bg="blue.500"/>
                                                            <Text fontSize="sm">{member.name}</Text>
                                                        </HStack>
                                                    ))}
                                                    <IconButton
                                                        variant="outline"
                                                        size="sm"
                                                        icon={<AddIcon/>}
                                                        aria-label="Добавить участника"
                                                    />
                                                </HStack>
                                            </VStack>
                                        )}
                                    </VStack>

                                    <Divider/>

                                    {/* Описание */}
                                    <VStack align="stretch" spacing={3}>
                                        <HStack justify="space-between">
                                            <HStack>
                                                <EditIcon w={4} h={4}/>
                                                <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                    Описание
                                                </Text>
                                            </HStack>
                                            {!isEditingDescription && (
                                                <Button
                                                    variant="outline"
                                                    size="sm"
                                                    onClick={() => setIsEditingDescription(true)}
                                                >
                                                    Редактировать
                                                </Button>
                                            )}
                                        </HStack>

                                        {isEditingDescription ? (
                                            <VStack spacing={3} align="stretch">
                                                <Textarea
                                                    value={description}
                                                    onChange={(e) => setDescription(e.target.value)}
                                                    minH="100px"
                                                    placeholder="Добавьте более подробное описание..."
                                                />
                                                <HStack>
                                                    <Button size="sm" colorScheme="blue">
                                                        Сохранить
                                                    </Button>
                                                    <Button
                                                        variant="outline"
                                                        size="sm"
                                                        onClick={() => setIsEditingDescription(false)}
                                                    >
                                                        Отмена
                                                    </Button>
                                                </HStack>
                                            </VStack>
                                        ) : (
                                            <Box
                                                bg="gray.50"
                                                borderRadius="lg"
                                                p={4}
                                                cursor="pointer"
                                                _hover={{bg: "gray.100"}}
                                                onClick={() => setIsEditingDescription(true)}
                                            >
                                                <Text fontSize="sm" color="gray.700">
                                                    {description || "Добавьте более подробное описание..."}
                                                </Text>
                                            </Box>
                                        )}
                                    </VStack>

                                    <Divider/>

                                    {/* Вложения */}
                                    {mockCard.attachments.length > 0 && (
                                        <VStack align="stretch" spacing={3}>
                                            <HStack>
                                                <AttachmentIcon w={4} h={4}/>
                                                <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                    Вложения
                                                </Text>
                                            </HStack>
                                            <VStack spacing={2} align="stretch">
                                                {mockCard.attachments.map((attachment) => (
                                                    <Flex key={attachment.id} justify="space-between" align="center" p={3}
                                                          bg="gray.50" borderRadius="lg">
                                                        <HStack spacing={3}>
                                                            <Box p={2} bg="white" borderRadius="md">
                                                                <AttachmentIcon w={4} h={4} color="gray.500"/>
                                                            </Box>
                                                            <VStack align="start" spacing={0}>
                                                                <Text fontSize="sm"
                                                                      fontWeight="medium">{attachment.name}</Text>
                                                                <Text fontSize="xs"
                                                                      color="gray.500">{attachment.size}</Text>
                                                            </VStack>
                                                        </HStack>
                                                        <Button variant="ghost" size="sm">
                                                            Скачать
                                                        </Button>
                                                    </Flex>
                                                ))}
                                            </VStack>
                                        </VStack>
                                    )}

                                    <Divider/>

                                    {/* Комментарии */}
                                    <VStack align="stretch" spacing={4}>
                                        <HStack>
                                            <ChatIcon w={4} h={4}/>
                                            <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                Активность
                                            </Text>
                                        </HStack>

                                        {/* Добавить комментарий */}
                                        <HStack align="start" spacing={3}>
                                            <Avatar size="sm" bg="green.500" name="Вы"/>
                                            <VStack flex="1" align="stretch">
                                                <Textarea
                                                    value={newComment}
                                                    onChange={(e) => setNewComment(e.target.value)}
                                                    placeholder="Написать комментарий..."
                                                    minH="80px"
                                                />
                                                <Flex justify="end">
                                                    <Button
                                                        size="sm"
                                                        colorScheme="blue"
                                                        // onClick={}
                                                        isDisabled={!newComment.trim()}
                                                    >
                                                        Отправить
                                                    </Button>
                                                </Flex>
                                            </VStack>
                                        </HStack>

                                        {/* Список комментариев */}
                                        <VStack spacing={4} align="stretch">
                                            {mockCard.comments.map((comment) => (
                                                <HStack key={comment.id} align="start" spacing={3}>
                                                    <Avatar size="sm" name={comment.initials} bg="blue.500"/>
                                                    <VStack flex="1" align="stretch">
                                                        <Box bg="white" border="1px solid" borderColor="gray.200"
                                                             borderRadius="lg" p={3}>
                                                            <HStack spacing={2} mb={2}>
                                                                <Text fontSize="sm"
                                                                      fontWeight="medium">{comment.author}</Text>
                                                                <Text fontSize="xs" color="gray.500">{comment.date}</Text>
                                                            </HStack>
                                                            <Text fontSize="sm" color="gray.700">{comment.text}</Text>
                                                        </Box>
                                                    </VStack>
                                                </HStack>
                                            ))}
                                        </VStack>
                                    </VStack>
                                </VStack>
                            </GridItem>

                            {/* Правая колонка - действия */}
                            <GridItem>
                                <VStack spacing={4} align="stretch">
                                    <VStack align="stretch" spacing={3}>
                                        <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                            Добавить к карточке
                                        </Text>
                                        <VStack spacing={2} align="stretch">
                                            <Button variant="outline" size="sm" leftIcon={<EditIcon/>}
                                                    justifyContent="start">
                                                Участники
                                            </Button>
                                            <Button variant="outline" size="sm" leftIcon={<EditIcon/>}
                                                    justifyContent="start" onClick={() => setIsManageLabelsModalOpen(true)}>
                                                Метки
                                            </Button>
                                            <Button variant="outline" size="sm" leftIcon={<CalendarIcon/>}
                                                    justifyContent="start">
                                                Срок
                                            </Button>
                                            <Button variant="outline" size="sm" leftIcon={<AttachmentIcon/>}
                                                    justifyContent="start">
                                                Вложение
                                            </Button>
                                        </VStack>
                                    </VStack>

                                    <Divider/>

                                    <VStack align="stretch" spacing={3}>
                                        <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                            Действия
                                        </Text>
                                        <VStack spacing={2} align="stretch">
                                            <Button variant="outline" size="sm" leftIcon={<EditIcon/>}
                                                    justifyContent="start">
                                                Изменить
                                            </Button>
                                            <Button variant="outline" size="sm" leftIcon={<ViewIcon/>}
                                                    justifyContent="start">
                                                Следить
                                            </Button>
                                            <Button variant="outline" size="sm" leftIcon={<TimeIcon/>}
                                                    justifyContent="start">
                                                Архивировать
                                            </Button>
                                        </VStack>
                                    </VStack>

                                    {mockCard.dueDate && (
                                        <>
                                            <Divider/>
                                            <VStack align="stretch" spacing={2}>
                                                <Text fontSize="sm" fontWeight="medium" color="gray.700">
                                                    Срок выполнения
                                                </Text>
                                                <Box bg="yellow.50" border="1px solid" borderColor="yellow.200"
                                                     borderRadius="lg" p={3}>
                                                    <HStack spacing={2}>
                                                        <CalendarIcon w={4} h={4} color="yellow.600"/>
                                                        <Text fontSize="sm" fontWeight="medium" color="yellow.800">
                                                            {mockCard.dueDate}
                                                        </Text>
                                                    </HStack>
                                                    <Text fontSize="xs" color="yellow.600" mt={1}>
                                                        Скоро истекает
                                                    </Text>
                                                </Box>
                                            </VStack>
                                        </>
                                    )}
                                </VStack>
                            </GridItem>
                        </Grid>
                    </ModalBody>
                </ModalContent>
            </Modal>

            {isManageLabelsModalOpen && cardId && boardId && (
                <ManageLabelsModal
                    isOpen={isManageLabelsModalOpen}
                    onClose={() => setIsManageLabelsModalOpen(false)}
                    cardId={cardId}
                    boardId={boardId}
                    currentCardLabels={cardLabels}
                    availableLabels={allBoardLabels}
                    onLabelsUpdated={handleLabelsUpdatedFromModal}
                />
            )}
        </>
    );
}