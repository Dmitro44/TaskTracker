import { Modal, ModalOverlay, ModalContent, ModalHeader, ModalCloseButton, ModalBody, Input, ModalFooter, Button } from "@chakra-ui/react";

interface CreateColumnModalProps {
    isOpen: boolean;
    onClose: () => void;
    newColTitle: string;
    setNewColTitle: (title: string) => void;
    onCreate: () => void;
}

export function CreateColumnModal({
    isOpen,
    onClose,
    newColTitle,
    setNewColTitle,
    onCreate
} : CreateColumnModalProps) {
    return (
        <Modal isOpen={isOpen} onClose={onClose} isCentered>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>Новая колонка</ModalHeader>
                <ModalCloseButton />
                <ModalBody>
                <Input
                    placeholder="Название колонки"
                    value={newColTitle}
                    onChange={(e) => setNewColTitle(e.target.value)}
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