"use client";

import {
    Box,
    Flex,
    VStack,
    Text,
    Heading,
    Button,
    useColorModeValue,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { getBoards, Board, createBoard } from "@/app/services/boards";
import { AddIcon } from "@chakra-ui/icons";
import CreateBoardModal from "./CreateBoardModal";
import { useRouter } from "next/navigation";


export default function DashboardPage() {
    const router = useRouter();

    const [isModalOpen, setModalOpen] = useState(false);
    const [boards, setBoards] = useState<Board[]>([]);
    useEffect(() => {
        async function loadBoards() {
            try {
                const data = await getBoards();
                setBoards(data);
            } catch (err) {
                console.error("Ошибка при загрузке досок:", err);
            }
        }

        loadBoards();
    }, []);

    const handleCreate = async ({ title, visibility } : { title: string; visibility: "public" | "private"}) => {
        try{
            await createBoard({
                title: title,
                isPublic: visibility === "public",
            });

            setModalOpen(false);

            setBoards(await getBoards());
        } catch (e) {
            console.error("Error creating board", e);
        }
    }

    const sidebarBg = useColorModeValue("gray.50", "gray.900");
    const activeMenuBg = useColorModeValue("blue.50", "blue.900");
    const contentBg = useColorModeValue("gray.100", "gray.800");

    return (
        <Flex minH="100vh" bg={contentBg}>
            {/* Левое меню */}
            <Box
                w={{ base: "180px", md: "220px" }}
                bg={sidebarBg}
                borderRight="1px solid"
                borderRightColor={useColorModeValue("gray.200", "gray.700")}
                py={8}
                px={4}
            >
                <VStack align="stretch" spacing={4}>
                    <Flex align="center" gap={2} fontWeight="bold" fontSize="xl" mb={4}>
                        <span>Tasker</span>
                    </Flex>
                    <Box
                        bg={activeMenuBg}
                        color="blue.500"
                        borderRadius="md"
                        px={3}
                        py={2}
                        fontWeight="500"
                        cursor="pointer"
                    >
                        Мои доски
                    </Box>
                </VStack>
            </Box>

            {/* Контент с досками */}
            <Box flex="1" p={8}>
                <Flex align="center" mb={4} gap={6}>
                    <Heading size="lg" mb={6}>
                        Мои доски
                    </Heading>
                    <Button
                        leftIcon={<AddIcon/>}
                        onClick={() => setModalOpen(true)}
                        colorScheme="blue"
                        variant="solid"
                        justifyContent="flex-start"
                        mb={5}
                    >
                        Создать доску
                    </Button>
                </Flex>
                <CreateBoardModal
                    isOpen={isModalOpen}
                    onClose={() => setModalOpen(false)}
                    onCreate={handleCreate}
                />
                <Flex wrap="wrap" gap={6}>
                    {boards.map((board) => (
                        <Box
                            key={board.id}
                            w="260px"
                            h="110px"
                            bg="white"
                            borderRadius="lg"
                            boxShadow="md"
                            p={4}
                            cursor="pointer"
                            _hover={{ boxShadow: "xl", transform: "scale(1.03)" }}
                            transition="all 0.2s"
                            display="flex"
                            flexDirection="column"
                            justifyContent="space-between"
                            onClick={() => router.push(`/b/${board.id}`)}
                        >
                            <Heading size="md">{board.title}</Heading>
                            <Text>{board.isPublic ? "Public" : "Private"}</Text>
                        </Box>
                    ))}
                </Flex>
                {boards.length === 0 && (
                    <Text color="gray.400" mt={8} textAlign="center">
                        У вас пока нет досок.
                    </Text>
                )}
            </Box>
        </Flex>
    );
}