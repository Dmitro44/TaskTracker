import { useState } from "react";
import {
  Modal, ModalOverlay, ModalContent, ModalHeader, ModalBody, ModalFooter, ModalCloseButton,
  Button, Input, FormControl, FormLabel, RadioGroup, Radio, Stack
} from "@chakra-ui/react";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  onCreate: (data: { title: string; visibility: "public" | "private" }) => void;
};

export default function CreateBoardModal({ isOpen, onClose, onCreate }: Props) {
  const [title, setTitle] = useState("");
  const [visibility, setVisibility] = useState<"public" | "private">("private");

  const handleCreate = () => {
    if (!title.trim()) return;
    onCreate({ title, visibility });
    setTitle("");
    setVisibility("private");
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Создать доску</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <FormControl mb={4}>
            <FormLabel>Название доски</FormLabel>
            <Input
              value={title}
              onChange={e => setTitle(e.target.value)}
              placeholder="Введите название"
              autoFocus
            />
          </FormControl>
          <FormControl>
            <FormLabel>Доступность</FormLabel>
            <RadioGroup
              value={visibility}
              onChange={val => setVisibility(val as "public" | "private")}
            >
              <Stack direction="row">
                <Radio value="public">Публичная</Radio>
                <Radio value="private">Приватная</Radio>
              </Stack>
            </RadioGroup>
          </FormControl>
        </ModalBody>
        <ModalFooter>
          <Button onClick={onClose} mr={3} variant="ghost">
            Отмена
          </Button>
          <Button colorScheme="blue" onClick={handleCreate} isDisabled={!title.trim()}>
            Создать
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}