import { Modal, ModalOverlay, ModalContent, ModalHeader, ModalCloseButton, ModalBody, Input, ModalFooter, Button } from "@chakra-ui/react";

interface CreateCardModalProps {
    isOpen: boolean;
    onClose: () => void;
    newCardTitle: string;
    setNewCardTitle: (title: string) => void;
    onCreate: () => void;
}

export function CreateCardModal({
    isOpen,
    onClose,
    newCardTitle,
    setNewCardTitle,
    onCreate
} : CreateCardModalProps) {
    return (
        <Modal isOpen={isOpen} onClose={onClose} isCentered>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>Новая карточка</ModalHeader>
                <ModalCloseButton />
                <ModalBody>
                <Input
                    placeholder="Название карточки"
                    value={newCardTitle}
                    onChange={(e) => setNewCardTitle(e.target.value)}
                    autoFocus
                    mb={2}
                    onKeyDown={(e) => {
                    if (e.key === "Enter") onCreate();
                    }}
                />
                </ModalBody>
                <ModalFooter>
                <Button colorScheme="blue" mr={3} onClick={onCreate}>
                    Создать
                </Button>
                <Button onClick={onClose}>Отмена</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    )
}