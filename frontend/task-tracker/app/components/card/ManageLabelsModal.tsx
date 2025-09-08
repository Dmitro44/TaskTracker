import {createLabel, Label} from "@/app/services/labels";
import { addLabel, removeLabel } from "@/app/services/cards";
import {useEffect, useState} from "react";
import {
    Text,
    Badge,
    Box, Checkbox, CheckboxGroup, HStack, Input,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent,
    ModalHeader,
    ModalOverlay,
    VStack, Button, ModalFooter, useToast
} from "@chakra-ui/react";
import {getContrastTextColor} from "@/app/utils/colorUtils";

interface ManageLabelsModalProps {
    isOpen: boolean;
    onClose: () => void;
    cardId: string;
    boardId: string;
    currentCardLabels: Label[];
    availableLabels: Label[];
    onLabelsUpdated: () => Promise<void>;
}

export default function ManageLabelsModal({
    isOpen,
    onClose,
    cardId,
    boardId,
    currentCardLabels,
    availableLabels,
    onLabelsUpdated
}: ManageLabelsModalProps) {
    const [selectedLabelIds, setSelectedLabelIds] = useState<string[]>([]);
    const [newLabelName, setNewLabelName] = useState("");
    const [newLabelColor, setNewLabelColor] = useState<string>("#CCCCCC");
    const [isCreatingLabel, setIsCreatingLabel] = useState(false);
    const [isLabelOperationInProgress, setIsLabelOperationInProgress] = useState(false);
    
    const toast = useToast();

    useEffect(() => {
        if (currentCardLabels) {
            setSelectedLabelIds(currentCardLabels.map(l => l.id));
        }
    }, [currentCardLabels, isOpen]);

    const handleLabelSelectionChange = async (newlySelectedIds: string[]) => {
        if (isLabelOperationInProgress || isCreatingLabel) return;
        
        setIsLabelOperationInProgress(true);
        
        const oldSelectedLabelIds = [...selectedLabelIds];
        
        let changedLabelId: string | null = null;
        let operationType: "add" | "remove" | null = null;
        
        if (newlySelectedIds.length > oldSelectedLabelIds.length) {
            changedLabelId = newlySelectedIds.find(id => !oldSelectedLabelIds.includes(id)) || null;
            operationType = "add";
        } else if (newlySelectedIds.length < oldSelectedLabelIds.length) {
            changedLabelId = oldSelectedLabelIds.find(id => !newlySelectedIds.includes(id)) || null;
            operationType = "remove";
        }
        
        if (changedLabelId && operationType) {
            setSelectedLabelIds(newlySelectedIds);

            try {
                if (operationType === 'add') {
                    console.log(changedLabelId);
                    await addLabel(cardId, changedLabelId);
                    toast({ title: "Метка добавлена", status: "success", duration: 2000, isClosable: true });
                } else {
                    await removeLabel(cardId, changedLabelId);
                    toast({ title: "Метка удалена", status: "success", duration: 2000, isClosable: true });
                }
                await onLabelsUpdated();
            } catch (error) {
                toast({
                    title: `Ошибка ${operationType === 'add' ? 'добавления' : 'удаления'} метки`,
                    description: error instanceof Error ? error.message : "Не удалось выполнить операцию.",
                    status: "error",
                    duration: 5000,
                    isClosable: true,
                });
                
                setSelectedLabelIds(oldSelectedLabelIds);
            }
        } else {
            setSelectedLabelIds(newlySelectedIds);
        }
        
        setIsLabelOperationInProgress(false);
    }

    const handleCreateNewLabel = async () => {
        if (!newLabelName) {
            toast({
                title: "Ошибка",
                description: "Название метки не может быть пустым.",
                status: "warning",
                duration: 3000,
                isClosable: true,
            });
            return;
        }

        if (!/^#[0-9A-F]{6}$/i.test(newLabelColor) && !/^#[0-9A-F]{3}$/i.test(newLabelColor)) {
            toast({
                title: "Неверный формат цвета",
                description: "Пожалуйста, введите корректный HEX-код цвета (например, #RRGGBB).",
                status: "warning",
                duration: 4000,
                isClosable: true,
            });
            return;
        }
        
        setIsCreatingLabel(true);
        
        try {
            await createLabel(boardId, newLabelName, newLabelColor);
            
            setNewLabelName("");
            setNewLabelColor("#CCCCCC")
            
            await onLabelsUpdated();
        } catch (error) {
            toast({
                title: "Ошибка создания метки.",
                description: error instanceof Error ? error.message : "Не удалось создать метку.",
                status: "error",
                duration: 5000,
                isClosable: true,
            });
        } finally {
            setIsCreatingLabel(false);
        }
    }

    return (
        <Modal isOpen={isOpen} onClose={onClose} size="xl">
            <ModalOverlay/>
            <ModalContent>
                <ModalHeader>Добавление метки</ModalHeader>
                <ModalCloseButton />
                <ModalBody>
                    <VStack spacing={4} align="stretch">
                        <Box>
                            <Text fontWeight="bold" mb={2}>Доступные метки для доски:</Text>
                            <CheckboxGroup
                                value={selectedLabelIds}
                                onChange={handleLabelSelectionChange}
                            >
                                <VStack align="stretch" spacing={2}>
                                    {availableLabels && availableLabels.map((label) => (
                                        <Checkbox 
                                            key={label.id} 
                                            value={label.id}
                                            size="lg"
                                        >
                                            <Badge 
                                                bg={label.color}
                                                color={getContrastTextColor(label.color)}
                                                fontSize="lg" mr={2}
                                            >
                                                {label.name}
                                            </Badge>
                                        </Checkbox>
                                    ))}
                                    {availableLabels && availableLabels.length === 0 && <Text>Для этой доски еще нет меток.</Text>}
                                </VStack>
                            </CheckboxGroup>
                        </Box>
                        <Box borderTopWidth="1px" pt={4}>
                            <Text fontWeight="bold" mb={2}>Создать новую метку для доски:</Text>
                            <HStack spacing={2}>
                                <Input
                                    placeholder="Название новой метки"
                                    value={newLabelName}
                                    onChange={(e) => setNewLabelName(e.target.value)}
                                />
                                <Input
                                    type="color"
                                    value={newLabelColor}
                                    onChange={(e) => setNewLabelColor(e.target.value)}
                                    width="80px"
                                    p={1}
                                />
                                <Button 
                                    onClick={handleCreateNewLabel}
                                    isDisabled={!newLabelName.trim()}
                                    isLoading={isCreatingLabel}
                                >
                                    Создать
                                </Button>
                            </HStack>
                        </Box>
                    </VStack>
                </ModalBody>
                <ModalFooter/>
            </ModalContent>
        </Modal>
    );
}